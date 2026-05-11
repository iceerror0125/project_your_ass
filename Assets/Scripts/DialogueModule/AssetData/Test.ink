// Kịch bản: Cuộc gặp gỡ bí mật tại trạm tàu
VAR speaker = ""
VAR side = ""

-> meeting_start

=== meeting_start ===
~ speaker = "A"
~ side = "left"
Cuối cùng cậu cũng đến. Tôi đã đợi ở đây hơn 2 tiếng rồi đấy. #A:annoy

~ speaker = "B"
~ side = "right"
Lộ trình có chút thay đổi, cảnh sát tuần tra gắt gao hơn tôi tưởng. #B:annoy

~ speaker = "A"
~ side = "left"
Vậy món đồ tôi yêu cầu... cậu có mang theo không? #A:surprise

+ [Có, đây là tài liệu]
    -> provide_info
+ [Tôi cần tiền trước]
    -> demand_money
+ [Thực ra... tôi đã làm mất nó]
    -> lost_item

=== provide_info ===
~ speaker = "B"
~ side = "right"
Đây. Tất cả nằm trong chiếc USB này. #B:happy

~ speaker = "A"
~ side = "left"
Tốt lắm. Cậu luôn làm việc rất chuyên nghiệp. #A:happy
-> ending_success

=== demand_money ===
~ speaker = "B"
~ side = "right"
Tiền trao cháo múc. Cậu biết luật mà, đúng không? #B:annoy

~ speaker = "A"
~ side = "left"
Cậu không tin tưởng tôi đến thế sao? #A:annoy

~ speaker = "B"
~ side = "right"
Ở cái thành phố này, tin tưởng là một món hàng xa xỉ. #B:annoy

* [Đưa tiền cho B]
    ~ speaker = "A"
    ~ side = "left"
    Được rồi, cầm lấy và đưa thứ đó cho tôi. #A:happy
    
    ~ speaker = "B"
    ~ side = "right"
    Rất vui được hợp tác. #B:happy
    -> provide_info
* [Đe dọa B]
    ~ speaker = "A"
    ~ side = "left"
    Cậu nên nhớ ai là người nắm quyền ở đây. Đưa nó ra ngay! #A:annoy
    
    ~ speaker = "B"
    ~ side = "right"
    Cậu đang tự làm khó mình đấy... #B:annoy
    -> escape_scene

=== lost_item ===
~ speaker = "B"
~ side = "right"
Có một chút rắc rối... Tôi đã bị phục kích trên đường tới đây. #B:surprise

~ speaker = "A"
~ side = "left"
Cái gì? Cậu có biết mình vừa gây ra họa lớn không? #A:annoy
Nếu thông tin đó lọt vào tay kẻ khác, cả hai chúng ta đều tiêu đời! #A:annoy

* [Xin lỗi A]
    ~ speaker = "B"
    ~ side = "right"
    Tôi thực sự xin lỗi, tôi sẽ tìm cách lấy lại nó. #B:annoy
    
    ~ speaker = "A"
    ~ side = "left"
    Hy vọng là cậu còn đủ mạng sống để thực hiện lời hứa đó. #A:annoy
    -> DONE
* [Đổ lỗi cho A]
    ~ speaker = "B"
    ~ side = "right"
    Nếu cậu không chọn cái địa điểm chết tiệt này thì chuyện đã không xảy ra! #B:annoy
    
    ~ speaker = "A"
    ~ side = "left"
    Đủ rồi! Biến khỏi mắt tôi ngay trước khi tôi đổi ý. #A:annoy
    -> DONE

=== escape_scene ===
~ speaker = "B"
~ side = "right"
Cuộc trò chuyện này kết thúc tại đây. #B:annoy

~ speaker = "A"
~ side = "left"
Đứng lại đó! #A:annoy
-> DONE

=== ending_success ===
~ speaker = "A"
~ side = "left"
Chúng ta sẽ liên lạc lại sau khi tôi kiểm tra xong nội dung. #A:happy

~ speaker = "B"
~ side = "right"
Tùy cậu. Tôi đi đây. #B:happy
-> DONE
