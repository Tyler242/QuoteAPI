namespace QuoteAPI.Models
{
    public class QuoteModelFactory
    {
        public Quote ConvertToQuote(QuoteUpdateModel updateModel, Quote quote)
        {
            quote.QuoteText = updateModel.QuoteText;
            quote.QuoteSource = updateModel.QuoteSource;
            quote.Tags = updateModel.Tags;
            quote.LastModified = DateTime.Now;

            return quote;
        }

        public Quote ConvertToQuote(QuoteCreationModel creationModel) => new()
        {
            QuoteText = creationModel.QuoteText,
            QuoteSource = creationModel.QuoteSource,
            Tags = creationModel.Tags,
            LastModified = DateTime.Now,
            Views = 0
        };
    }
}
