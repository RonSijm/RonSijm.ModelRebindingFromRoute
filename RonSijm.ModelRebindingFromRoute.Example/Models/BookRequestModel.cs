using Microsoft.AspNetCore.Mvc;

namespace RonSijm.ModelRebindingFromRoute.Example.Models
{
    public class BookRequestModel
    {
        [FromRoute]
        public string BookId { get; set; }

        [FromRoute]
        public string PageId { get; set; }
    }
}
