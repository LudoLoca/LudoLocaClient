namespace Client.Models
{
    public class GameViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int? YearPublished { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public int? PlayTimeMinutes { get; set; }
        public string? Designer { get; set; }
        public DateTime? CreatedAt { get; set; }

        // gêneros vinculados ao jogo
        public List<GenreViewModel> Genres { get; set; } = new();

        // todos os gêneros disponíveis
        public List<GenreViewModel> AllGenres { get; set; } = new();
    }
}
