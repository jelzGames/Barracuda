import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import i18n from "i18next";
import enUS from "./languages/en-US.json";
import esMX from "./languages/es-MX.json";
import esUS from "./languages/es-US.json"

i18n
    .use(LanguageDetector)
    .use(initReactI18next) 
    .init({
        resources: { 
            "es-US": {
                common: esUS
              },
            "en-US": {
                common: enUS
              },
            "es-MX": {
                common: esMX
            },
            "fi-FI": {
                common: esMX
            }
        },
        keySeparator: false, 
        interpolation: {
            escapeValue: false 
        },
    });


export default i18n;