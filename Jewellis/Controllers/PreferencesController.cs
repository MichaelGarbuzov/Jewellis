using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Jewellis.Controllers
{
    public class PreferencesController : Controller
    {
        private readonly ClientThemeService _clientTheme;
        private readonly ClientCurrencyService _clientCurrency;

        public PreferencesController(ClientThemeService clientTheme, ClientCurrencyService clientCurrency)
        {
            _clientTheme = clientTheme;
            _clientCurrency = clientCurrency;
        }

        #region AJAX Actions

        [Route("/Preferences/UpdateClientTheme")]
        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> UpdateClientTheme([FromBody] string themeId)
        {
            if (string.IsNullOrEmpty(themeId))
                return Json(false);

            await _clientTheme.SetAsync(themeId);
            return Json(true);
        }

        [Route("/Preferences/UpdateClientCurrency")]
        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> UpdateClientCurrency([FromBody] string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                return Json(false);

            await _clientCurrency.SetAsync(currencyCode);
            return Json(true);
        }

        #endregion

    }
}
