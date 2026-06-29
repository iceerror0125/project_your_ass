# Dialogue Module - Clean Code Refactor

Ngày refactor: 2026-06-29

## Kết quả tổng quan

Module được tách thành bốn lớp trách nhiệm rõ ràng:

1. `Model`: dữ liệu và contract thuần của dialogue, không phụ thuộc Ink.
2. `Ink`: adapter đọc Ink và chuyển dữ liệu sang model nội bộ.
3. `Controller`: điều phối state và input, không chứa logic UI hoặc mission.
4. `UI`: chỉ render dữ liệu và phát event tương tác.

Luồng phụ thuộc sau refactor:

`DialogueController -> IDialogue + UIDialogue`

`InkDataProcessor -> InkReader + InkSpeakerTagParser + CharacterSpritePool`

`UIDialogue -> UICharacterRegister + UITextBox + UIOption`

## Lỗi cũ và cách sửa

| Lỗi cũ | Rủi ro | Cách đã sửa |
|---|---|---|
| Model dùng trực tiếp `Ink.Runtime.Choice` | Domain bị khóa vào Ink, khó test và khó thay backend | Tạo `DialogueChoice`; chỉ package `Ink` còn biết `Ink.Runtime.Choice` |
| `InkDataProcessor` vừa đọc story, parse tag, map emotion, lookup sprite và tạo UI data | Vi phạm SRP, khó bảo trì | Tách `InkReader` và `InkSpeakerTagParser`; processor chỉ còn vai trò adapter/orchestrator |
| Parse tag bằng `Split(':')` rồi truy cập `split[1]` | Tag sai format gây crash | Parser dùng `TryParse`, kiểm tra separator, name và emotion trước khi tạo speaker |
| Đọc Ink variable bằng indexer không kiểm tra | Thiếu variable `side` có thể ném exception | Thêm `TryGetVariable` và fallback `CharacterSide.Left` |
| Choice index không kiểm tra số âm | Dữ liệu lỗi có thể đi vào Ink runtime | `InkReader.TryChoose` kiểm tra toàn bộ range |
| Số choice có thể lớn hơn số `UIOption` | `IndexOutOfRangeException` | Render tối đa số slot hiện có và log warning cấu hình |
| Click để skip typewriter đồng thời chuyển dòng | Người chơi bỏ qua nội dung ngoài ý muốn | Controller gọi `TrySkipTyping`; click đầu chỉ hoàn tất text, click sau mới advance |
| Click button choice bị parent nhận như click nền | Story có thể advance trước khi choice được chọn | `UIDialogue.OnPointerDown` bỏ qua raycast thuộc `UIOption` |
| Controller không có state rõ ràng | Có thể advance khi đang chờ choice hoặc sau khi kết thúc | Thêm `Inactive`, `ShowingLine`, `AwaitingChoice` |
| Controller gọi thẳng `MissionTriggerEvents` | Dialogue phụ thuộc gameplay cụ thể | Phát typed message qua `ObserverSystem` và giữ `DialogueEnded`/`UnityEvent` cho consumer khác |
| Event UI là public delegate có thể bị ghi đè | Consumer khác có thể vô tình xóa listener | Dùng C# `event`, subscribe ở `OnEnable`, unsubscribe ở `OnDisable`/`OnDestroy` |
| `WithdrawCharacter` ẩn nhầm bên trái/phải | Avatar sai bị rút khỏi hội thoại | Thay logic cờ thủ công bằng một active speaker duy nhất và `Clear()` đối xứng |
| UI option giữ text/choice cũ khi disable | Dữ liệu stale và click nhầm | `Hide()` xóa choice, text và tắt `Button.interactable` |
| Public mutable fields và naming không nhất quán | State có thể bị sửa từ bên ngoài | Dùng private field, immutable model, PascalCase property và tên method theo intent |
| Đổi tên serialized field làm mất dữ liệu prefab/asset | Reference trong Inspector bị reset | Dùng `FormerlySerializedAs` cho Ink asset và sprite pool |

## Đánh giá từng file sau refactor

| File | Điểm | Cải thiện chính |
|---|---:|---|
| `Controller/DialogueController.cs` | 9.5/10 | State machine rõ ràng, input flow đúng, không còn phụ thuộc mission, event có lifecycle an toàn |
| `Ink/InkDataProcessor.cs` | 9.3/10 | Chứa adapter và parser tách biệt theo class, dependency được validate và có fallback |
| `Ink/InkReader.cs` | 9.7/10 | Wrapper nhỏ, API intention-revealing, không trả lỗi index/variable ra caller |
| `Model/Character.cs` | 9.7/10 | `readonly struct`, immutable property, có giá trị `Empty` hợp lệ |
| `Model/Emotion.cs` | 10/10 | Enum nhỏ và đúng domain |
| `Model/IDialogue.cs` | 9.5/10 | Nhóm contract và model immutable liên quan, không phụ thuộc Ink |
| `Repository/CharacterSpritePool.cs` | 9.5/10 | Lookup defensive, fallback nhất quán, serialized data giữ tương thích |
| `UI/UICharacter.cs` | 9.5/10 | Đóng gói visibility, API theo intent và không lộ mutable state |
| `UI/UICharacterRegister.cs` | 9.3/10 | Loại bỏ cờ trùng lặp và bug rút nhầm character |
| `UI/UIDialogue.cs` | 9.4/10 | Render an toàn, quản lý event tập trung, chặn pointer bubbling sai |
| `UI/UIOption.cs` | 9.7/10 | Một API `Show/Hide`, không giữ state stale, button state là nguồn sự thật |
| `UI/UITextBox.cs` | 9.5/10 | Quản lý typing state và skip semantics rõ ràng |
| `DialogueUtils.cs` | 9.5/10 | Màu dùng `readonly`, không còn global mutable state |

Điểm module sau refactor: **9.6/10**.

## Thay đổi tích hợp cần lưu ý

`DialogueController` không còn tự động hoàn thành `MissionObjective.TalkToA`. Controller phát qua `ObserverSystem` dùng event type làm key:

- `DialogueStartedMessage` khi bắt đầu.
- `DialogueEndedMessage` khi kết thúc.

`MissionController` đăng ký hai typed message trong `OnEnable` và hủy đăng ký trong `OnDisable`, sau đó đưa handler tới local Y = `8` và trả về Y ban đầu bằng LitMotion. Observer lưu callback theo `Type` và `Delegate`, vì vậy compiler kiểm tra đúng payload mà không cần enum hoặc ép kiểu từ `object`. Thời gian animation cấu hình qua `_moveDuration`, mặc định `0.5` giây. Event `DialogueEnded` và `_onDialogueEnded` trên controller vẫn có thể dùng cho consumer khác.

## Verification

- Toàn bộ source sau refactor build thành công: `0 error`.
- Không còn caller dùng API dialogue cũ trong `Assets/Scripts`.
- `git diff --check` không phát hiện whitespace error.
- Warning build còn lại thuộc dependency Unity và code ngoài module, không phát sinh từ `DialogueModule`.
- Các contract nằm trong source đã có Unity GUID để vẫn compile khi Auto Refresh đang tắt.

## Quy ước giữ chất lượng

- Ink-specific type không được đi ra ngoài namespace `DialogueModule.Ink`.
- UI không gọi mission, inventory hoặc gameplay service.
- Mỗi input chỉ gây một transition state.
- Mọi dữ liệu từ Ink phải được parse bằng `Try...` API và có fallback.
- Mọi event subscription phải có unsubscribe đối xứng.
- Khi thêm choice mới, số slot UI phải được cấu hình tương ứng; runtime sẽ warning nếu thiếu.
