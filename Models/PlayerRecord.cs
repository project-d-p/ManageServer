using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageServer.Models
{
    public class PlayerRecord
    {
        [Key]
        public int RecordId { get; set; } // 전적 ID

        [ForeignKey("User")]
        public int UserId { get; set; } // User 테이블의 외래키

        public User? User { get; set; } // 관련 User 엔티티

        public int Rank { get; set; } // 순위

        public int TotalScore { get; set; } // 총 점수

        public int CapturedNum { get; set; } // 잡은 동물의 수

        public PlayerRole Role { get; set; } // 직업 열거형
    }

    // 플레이어의 직업을 나타내는 Enum
    public enum PlayerRole
    {
        Job1 = 0,
        Job2 = 1,
        Job3 = 2,
    }
}
