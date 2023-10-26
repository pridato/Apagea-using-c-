using System;
using System.Text.Json;

namespace Apagea2023.Models
{
	public class APIRestMessage
	{
        #region ... propiedades ... (formato JSON igual k en GeoApi)

        public String Update_date { get; set; } = "";
        public int Size { get; set; }
        public List<JsonElement> Data { get; set; } = new List<JsonElement>();
        public String Warning { get; set; } = "";

        #endregion
    }
}

