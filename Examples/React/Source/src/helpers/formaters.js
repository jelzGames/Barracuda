export const NumberFormat = (value, maxDecimals = 2, i18n, t) => {
    var format = new Intl.NumberFormat(i18n.language, {
        maximumFractionDigits: maxDecimals
    })
    .format(value);

    if (t("PositionCurrencySymbol") === "left") {
        format = t("CurrencySymbol") + " " + format;
    }
    else {
        format = format + " " + t("CurrencySymbol");
    }

    return format;
};