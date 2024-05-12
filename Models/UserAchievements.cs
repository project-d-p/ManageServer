using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageServer.Models
{
    public class UserAchievements
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // 업적 데이터의 고유 ID

        [ForeignKey("User")]
        public int UserId { get; set; }  // User 테이블의 외래 키

        public User User { get; set; }  // 관련 User 엔티티

        [Required]
        public int TotalGamesPlayed { get; set; }  // 지금까지 총 게임 횟수

        [Required]
        public int TotalGamesWon { get; set; }  // 지금까지 이긴 게임 횟수

        [Required]
        public int TotalEnemiesCaptured { get; set; }  // 지금까지 포획한 적의 수
    }
}
