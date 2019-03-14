import { Component, Vue } from 'vue-property-decorator';
import { SampleDataClient, WeatherForecast } from '@/service';

@Component
export default class Weather extends Vue {
  public forecasts: WeatherForecast[] = [];

  public created() {
    const client = new SampleDataClient();
    client.weatherForecasts().then((response) => {
      if (response) {
        this.forecasts = response;
      }
    });
  }
}
