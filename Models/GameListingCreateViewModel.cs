namespace Client.Models
{
    public class GameListingCreateViewModel
    {
        public GameListingViewModel GameListing { get; set; } = new();
        public List<GameViewModel> AvailableGames { get; set; } = new();
    }
}