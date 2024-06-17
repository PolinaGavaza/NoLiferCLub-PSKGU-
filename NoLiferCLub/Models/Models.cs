using System;
using System.Collections.Generic;

namespace NoLiferClub
{
    public class Player
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }


        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }

    public class Tournament
    {
        public int TournamentId { get; set; }


        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int GameId { get; set; } // Связь с таблицей Games

        public TournamentType Type { get; set; }

        public bool HasPrize { get; set; }

        public int ComputerRoomId { get; set; } // Связь с таблицей ComputerRooms

        public List<Result> Results { get; set; } = new List<Result>();
    }

    public class Game
    {
        public int GameId { get; set; }


        public string Name { get; set; }


        public GameCategory Category { get; set; }
    }

    public class ComputerRoom
    {
        public int ComputerRoomId { get; set; }


        public string Name { get; set; }


        public string Location { get; set; }

        public int ComputerCount { get; set; }
    }

    public class Result
    {
        public int ResultId { get; set; }

        public int PlayerId { get; set; } // Связь с таблицей Player

        public int TournamentId { get; set; } // Связь с таблицей Tournaments

        public int Place { get; set; }

        public int Points { get; set; }
    }

    public enum TournamentType
    {
        Amateur,
        Official
    }

    public enum GameCategory
    {
        RTS,
        ККИ,
        FPS,
        MOBA,
        SportSim
    }
}