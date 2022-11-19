using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class BillableAccountByPaymentAccount
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("__isBillableAccount")]
	public string IsBillableAccount { get; set; }

	[JsonProperty("is_using_ec")]
	public bool IsUsingEc { get; set; }

	[JsonProperty("payment_modes")]
	public List<string> PaymentModes { get; set; }

	[JsonProperty("billing_payment_account")]
	public BillingPaymentAccount BillingPaymentAccount { get; set; }

	[JsonProperty("is_on_flex_billing")]
	public bool IsOnFlexBilling { get; set; }

	[JsonProperty("viewer_permissions")]
	public ViewerPermissions ViewerPermissions { get; set; }

	[JsonProperty("risk_restriction")]
	public object RiskRestriction { get; set; }

	[JsonProperty("owning_business")]
	public OwningBusiness OwningBusiness { get; set; }

	[JsonProperty("billable_account_tax_info")]
	public BillableAccountTaxInfo BillableAccountTaxInfo { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("india_network_tokenization_migration_status")]
	public string IndiaNetworkTokenizationMigrationStatus { get; set; }

	[JsonProperty("currency")]
	public string Currency { get; set; }

	[JsonProperty("account_status")]
	public string AccountStatus { get; set; }

	[JsonProperty("spending_info")]
	public SpendingInfo SpendingInfo { get; set; }

	[JsonProperty("upsell_enabled")]
	public bool UpsellEnabled { get; set; }

	[JsonProperty("flex_billing_config")]
	public object FlexBillingConfig { get; set; }

	[JsonProperty("coupon_balance")]
	public CouponBalance CouponBalance { get; set; }

	[JsonProperty("prepay_balance")]
	public object PrepayBalance { get; set; }

	[JsonProperty("account_balance")]
	public AccountBalance AccountBalance { get; set; }

	[JsonProperty("policy_block")]
	public bool PolicyBlock { get; set; }

	[JsonProperty("next_billing_statement_info")]
	public object NextBillingStatementInfo { get; set; }

	[JsonProperty("billing_threshold_currency_amount")]
	public BillingThresholdCurrencyAmount BillingThresholdCurrencyAmount { get; set; }

	[JsonProperty("next_bill_date")]
	public int NextBillDate { get; set; }

	[JsonProperty("qe_on_payment_account_by_string")]
	public bool QeOnPaymentAccountByString { get; set; }

	[JsonProperty("is_eos_with_30_days")]
	public bool IsEosWith30Days { get; set; }

	[JsonProperty("is_eos_with_anniversary")]
	public bool IsEosWithAnniversary { get; set; }

	[JsonProperty("is_eos_with_30_days_v2")]
	public bool IsEosWith30DaysV2 { get; set; }

	[JsonProperty("is_eos_with_anniversary_v2")]
	public bool IsEosWithAnniversaryV2 { get; set; }

	[JsonProperty("bv_enabled")]
	public bool BvEnabled { get; set; }

	[JsonProperty("ad_campaign_groups")]
	public AdCampaignGroups AdCampaignGroups { get; set; }

	[JsonProperty("account_level_coupon_balance")]
	public AccountLevelCouponBalance AccountLevelCouponBalance { get; set; }

	[JsonProperty("campaign_level_coupon_balance")]
	public CampaignLevelCouponBalance CampaignLevelCouponBalance { get; set; }

	[JsonProperty("is_reauth_restricted")]
	public bool IsReauthRestricted { get; set; }

	[JsonProperty("prepay_on_postpay_gk")]
	public bool PrepayOnPostpayGk { get; set; }

	[JsonProperty("can_update_mi_billing_info_nexus")]
	public bool CanUpdateMiBillingInfoNexus { get; set; }

	[JsonProperty("updated_risk_notifications_enabled")]
	public bool UpdatedRiskNotificationsEnabled { get; set; }

	[JsonProperty("billing_flags")]
	public List<object> BillingFlags { get; set; }

	[JsonProperty("billing_permissions")]
	public List<string> BillingPermissions { get; set; }

	[JsonProperty("fbin_network_tokenization_verify_consent_flow")]
	public bool FbinNetworkTokenizationVerifyConsentFlow { get; set; }

	[JsonProperty("is_eligible_for_auto_refund")]
	public bool IsEligibleForAutoRefund { get; set; }

	[JsonProperty("billing_messages_to_skip")]
	public List<object> BillingMessagesToSkip { get; set; }

	[JsonProperty("balance")]
	public Balance Balance { get; set; }

	[JsonProperty("has_processing_payments")]
	public bool HasProcessingPayments { get; set; }

	[JsonProperty("is_hybrid_payment_enabled")]
	public bool IsHybridPaymentEnabled { get; set; }

	[JsonProperty("is_sdc_restricted")]
	public bool IsSdcRestricted { get; set; }

	[JsonProperty("risk_restricted_state")]
	public string RiskRestrictedState { get; set; }

	[JsonProperty("prisk_restrictions")]
	public List<object> PriskRestrictions { get; set; }

	[JsonProperty("billing_aymt_enabled")]
	public bool BillingAymtEnabled { get; set; }

	[JsonProperty("aymt_billing_megaphone_channel")]
	public object AymtBillingMegaphoneChannel { get; set; }

	[JsonProperty("refund_status")]
	public string RefundStatus { get; set; }

	[JsonProperty("invoice_billing_info_update_config")]
	public object InvoiceBillingInfoUpdateConfig { get; set; }

	[JsonProperty("active_survey_integration_point_for_billing_ui_viewer")]
	public ActiveSurveyIntegrationPointForBillingUiViewer ActiveSurveyIntegrationPointForBillingUiViewer { get; set; }
}
