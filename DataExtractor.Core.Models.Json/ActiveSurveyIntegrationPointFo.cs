using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class ActiveSurveyIntegrationPointForBillingUiViewer
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("survey_session")]
	public object SurveySession { get; set; }

	[JsonProperty("__module_operation_BillingNexusCardsRenderer_billableAccount_active_survey_integration_point_for_billing_ui_viewer")]
	public ModuleOperationBillingNexusCardsRendererBillableAccountActiveSurveyIntegrationPointForBillingUiViewer ModuleOperationBillingNexusCardsRendererBillableAccountActiveSurveyIntegrationPointForBillingUiViewer { get; set; }

	[JsonProperty("__module_component_BillingNexusCardsRenderer_billableAccount_active_survey_integration_point_for_billing_ui_viewer")]
	public ModuleComponentBillingNexusCardsRendererBillableAccountActiveSurveyIntegrationPointForBillingUiViewer ModuleComponentBillingNexusCardsRendererBillableAccountActiveSurveyIntegrationPointForBillingUiViewer { get; set; }
}
