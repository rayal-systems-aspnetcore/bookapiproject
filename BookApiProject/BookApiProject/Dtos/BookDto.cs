using System;

namespace BookApiProject.Dtos {
    public class BookDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
