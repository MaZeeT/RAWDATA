/*using automapper;
using databaseservice;
using microsoft.aspnetcore.mvc;

namespace webservice.controllers
{
    [apicontroller]
    [route("api/search")]
    public class searchcontroller : controllerbase
    {
        private idataservice _dataservice;
        private imapper _mapper;

        public searchcontroller(
            idataservice dataservice,
            imapper mapper)
        {
            _dataservice = dataservice;
            _mapper = mapper;
        }


        [httpget("{searchstring}")]
        public actionresult search(string searchstring)
        {
            var search = _dataservice.search(searchstring);

            //var result = createresult(categories);

            return ok(search);
        }


        ///////////////////
        //
        // helpers
        //
        //////////////////////



    }
}
*/