using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Security2.Database.Entities
{
    public class News
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <inheritdoc />
        public News(string title, string content)
        {
            Id = Guid.NewGuid();
            Title = title;
            Content = content;
            CreatedDate = DateTime.Now;
        } 
    }
}
