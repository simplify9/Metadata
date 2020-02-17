using SW.Content;
using SW.Content.Search.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{


    public class IndexService
    {
        readonly IIndexRepo _repo;
        public IndexService(IIndexRepo repo)
        {
            _repo = repo;
        }

        public UpdateIndexCommand CreateUpdateCommand(object key, object source)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var docType = new DocumentType(source.GetType());
            var docSource = new DocumentSource(docType, ContentFactory.Default.CreateFrom(key));
            
            return new UpdateIndexCommand(docSource, source);
        }



        public DropIndexCommand CreateDropCommand(object key, object source)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var docType = new DocumentType(source.GetType());
            var docSource = new DocumentSource(docType, ContentFactory.Default.CreateFrom(key));
            return new DropIndexCommand(docSource);
        }

       
        
        public async Task Handle(params IIndexCommand[] commands)
        {
            // update document(s) tokens

            //var tokens = commands
            //    .OfType<UpdateIndexCommand>()
            //    .SelectMany(u => GetTokens(u.Source, u.SourceData));

            //if (tokens.Any()) await _repo.SaveTokens(tokens.ToArray());

            var updates = commands
                .OfType<UpdateIndexCommand>().Select(cmd=>new Document {Data = cmd.SourceData,Source = cmd.Source});
           
            if (updates.Any()) await _repo.UpdateDocuments(updates.ToArray());

            // drop documents

            var deletes = commands.OfType<DropIndexCommand>().Select(cmd => cmd.Source);

            if (deletes.Any()) await _repo.DeleteDocuments(deletes.ToArray());
            
        }
        
      
    }
}
