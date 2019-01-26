import IWeatherForecast from '../types/WeatherForecast';
import dotenv from 'dotenv';

export default {


search: (cityOrPlz: string): Promise<any> => {
        const siteUrl = process.env.VUE_APP_WEBAPI_URL;
        const host = `${siteUrl}`;
        const path = 'api/WeatherData/WeatherForecasts';
        const api = `${host}${path}?cityOrZip=${cityOrPlz}`;
        const callWS = fetch(api);

        return callWS;
},
};
