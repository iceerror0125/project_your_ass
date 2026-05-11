namespace EntityModule
{
    public class EntityStat
    {
        public int hp { get; private set; } = 100;
        public float speed { get; private set; } = 5;
        
        public float jumpForce { get; private set; } = 2.0f;
        private const int MAX_HP = 100;
        private const int MIN_HP = 0;

        public void SetHp(int newValue)
        {
            switch (newValue)
            {
                case > MAX_HP:
                    this.hp = MAX_HP;
                    return;
                case < MIN_HP:
                    this.hp = MIN_HP;
                    return;
                default:
                    this.hp = newValue; break;
            }
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }
    }
}