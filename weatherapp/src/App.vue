<template>
  <div id="app">
    <div class="container">
      <div class="row mx-auto">
        <!-- Search input section -->
        <section class="col-sm-12 pt-3 px-0">
          <vs-input @search="doSearch"></vs-input>
        </section>
        <!-- Results section -->
        <section  class="col-sm-12 pt-3 px-0">
          <vs-results :weatherResults="weatherResultsMy" :weatherResultsError="weatherResultsErrorMy"></vs-results>
        </section>
      </div>
     </div>
  </div>
</template>

<script lang="ts">
  import { Component, Prop, Vue } from 'vue-property-decorator';
  import VsResults from './components/vs-res.vue';
  import VsInput from './components/vs-input.vue';
  import voyagerApi from './components/search-api';
  import IWeatherForecast from './types/WeatherForecast';

  @Component({
    components: {
      VsResults,
      VsInput,
    },
  })


  export default class App extends Vue {
    public weatherResultsMy: IWeatherForecast = {};
    public weatherResultsErrorMy: string = '';

  
    public async doSearch(cityOrPlz: string) {
      const qq = await voyagerApi.search(cityOrPlz);

      if (!qq.ok)
      {
        this.weatherResultsErrorMy = qq;
        this.weatherResultsMy = {};
        //throw new Error(qq.message);
      }
      else
      {
        const data = await qq.json();
        this.weatherResultsMy = data as IWeatherForecast;
        this.weatherResultsErrorMy = '';
      }
    }
  }
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
