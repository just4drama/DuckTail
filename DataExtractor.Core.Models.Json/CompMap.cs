using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class CompMap
{
	[JsonProperty("QPLInspector")]
	public QPLInspector QPLInspector { get; set; }

	[JsonProperty("CometRainbowNativeSurveyDialog.react")]
	public CometRainbowNativeSurveyDialogReact CometRainbowNativeSurveyDialogReact { get; set; }

	[JsonProperty("ODS")]
	public ODS ODS { get; set; }

	[JsonProperty("ExceptionDialog")]
	public ExceptionDialog ExceptionDialog { get; set; }

	[JsonProperty("React")]
	public React React { get; set; }

	[JsonProperty("ReactDOM")]
	public ReactDOM ReactDOM { get; set; }

	[JsonProperty("KeyEventTypedLogger")]
	public KeyEventTypedLogger KeyEventTypedLogger { get; set; }

	[JsonProperty("WebSpeedInteractionsTypedLogger")]
	public WebSpeedInteractionsTypedLogger WebSpeedInteractionsTypedLogger { get; set; }

	[JsonProperty("PerfXSharedFields")]
	public PerfXSharedFields PerfXSharedFields { get; set; }
}
