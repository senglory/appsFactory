export interface IReading {
    dateFormatted: string;
    temperatureC: number;
    humidity: number;
    windSpeed: number;
}

export default interface IWeatherForecast {
    city?: string;
    readings?: IReading[];
}
