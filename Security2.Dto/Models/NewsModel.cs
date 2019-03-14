using System;
using System.Collections.Generic;
using System.Text;

namespace Security2.Dto.Models
{
    public class NewsModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <inheritdoc />
        public NewsModel(Guid id, string title, string content, DateTime createdDate)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedDate = createdDate;
        }
    }
}
