function addCookie(key, value) {
    ///<summary>Adds or updates a cookie.</summary>
    /// <param name="key" type="String">Name of the cookie.</param>
    /// <param name="value" type="Object">Value of the cookie.</param>
    $.cookie(key,value)
};

function removeCookie(key) {
    ///<summary>Removes a cookie.</summary>
    /// <param name="key" type="String">Name of the cookie.</param>
    $.removeCookie(key);
};

function getCookie(key) {
	///<summary>Retrieves a cookie.</summary>
	/// <param name="key" type="String">Name of the cookie.</param>
	return $.cookie(key);
}