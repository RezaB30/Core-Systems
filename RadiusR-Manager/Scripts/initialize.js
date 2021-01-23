$(document).ready(function () {
    initializeNavbar();
    initializePartial("#page-contents");
    initializePartial("#search-slider");
    SetupLinearDiagrams("#page-contents");
    SetupPieCharts();
    SetupWebPhone();

    initializeConfirm('body');
    initializeResponseBoxes();
});

SetOnlineCount();

function initializePartial(containerId) {
    SetupSelectLists(containerId);
    SetupMultiselectList(containerId);
    SetupClearSearchs(containerId);
    SetupMultiButtons(containerId);
    SetupAddressDetails(containerId);
    SetupAddressEditors(containerId);
    SetupPhoneNoEditors(containerId);
    SetupTextListEditors(containerId);
    SetupTrafficLimits(containerId);
    SetupTransferLimits(containerId);
    SetupServiceRateTimeTable(containerId);
    SetupAddressSearch(containerId);
    SetupPhoneNoLists(containerId);
    SetupSubscriptionAddFee(containerId);
    SetupCheckbuttons(containerId);
    SetupTelekomTariffSelector(containerId);
    SetupFileUploads(containerId);
    SetupMultiFileUploads(containerId);
}