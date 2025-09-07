using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class GameListingCreateViewModel
    {
        [Required(ErrorMessage = "Escolha um jogo")]
        public Guid SelectedGameId { get; set; }

        [Required]
        public string PricePerDay { get; set; }

        public string ConditionNotes { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public DateTime? CreatedAt { get; set; }

        // Lista preenchida pelo controller (para popular o select)
        public List<GameViewModel> AvailableGames { get; set; } = new();
    }
}
