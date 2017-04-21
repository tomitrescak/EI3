//using Ei.Persistence;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System.IO;

//namespace Ei.Data.Yaml
//{
//    public static class JsonInstitutionFactory
//    {
//        public static void Save(InstitutionDao dao)
//        {
//            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
//             {
//                 Formatting = Formatting.Indented,
//                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
//                 NullValueHandling = NullValueHandling.Ignore
//             };

//            var json = JsonConvert.SerializeObject(dao);
//            File.WriteAllText("ei.json", json);
//        }
//    }
//}
