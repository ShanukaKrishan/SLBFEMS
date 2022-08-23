using System.Collections.Generic;
using Newtonsoft.Json;

namespace SLBFEMS.ViewModels.CVParser
{
    public class CVParserExternalResponseViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        [JsonProperty("email")]
        public string Email { get; set; } = null;

        [JsonProperty("address")]
        public string Address { get; set; } = null;

        [JsonProperty("education")]
        public List<CVParserExternalResponseEducatonViewModel> Education { get; set; } = null;

        [JsonProperty("experience")]
        public List<CVParserExternalResponseExperienceViewModel> Affiliation { get; set; } = null;

        [JsonProperty("skills")]
        public string[] Skills { get; set; } = null;
    }


    public class CVParserExternalResponseEducatonViewModel
    {
        [JsonProperty("dates")]
        public List<string> Dates { get; set; } = null;

        [JsonProperty("name")]
        public string Name { get; set; } = null;
    }

    public class CVParserExternalResponseExperienceViewModel
    {
        [JsonProperty("dates")]
        public List<string> Dates { get; set; } = null;

        [JsonProperty("location")]
        public string Location { get; set; } = null;

        [JsonProperty("organization")]
        public string Organization { get; set; } = null;

        [JsonProperty("title")]
        public string Title { get; set; } = null;
    }
}
