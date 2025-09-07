namespace Client.Models
{
    public class GameListingViewModel
    {
        public Guid Id { get; set; }
        public string ConditionNotes { get; set; } = string.Empty;
        public string PricePerDay { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public DateTime? CreatedAt { get; set; }

        // Objeto específico para o jogo
        public GameSummary Game { get; set; } = new();

        // Objeto específico para o usuário
        public UserSummary OwnerUser { get; set; } = new();

        public class GameSummary
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
        }

        public class UserSummary
        {
            public Guid Id { get; set; }
            public string Email { get; set; } = string.Empty;
        }
    }
}