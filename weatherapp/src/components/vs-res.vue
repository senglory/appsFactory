<template>
    <div v-if="weatherResults.city">
        <span><b>{{ weatherResults.city }}</b></span>

        <b-list-group v-for="item in weatherResults.readings" :key="item.dateFormatted">
            <b-list-group-item class="flex-column align-items-start">
                <div class="d-flex w-100 justify-content-between">
                    <div class="container">
                        <div class="row">
                            <div class="col-3">
                                <h5 class="mb-1">{{item.dateFormatted}}</h5>
                            </div>
                            <div class="col-5">
                                <div class="row">
                                    <div class="col-4">Temperature, C</div>
                                    <div class="col-4">{{item.temperatureC}}</div>
                                </div>
                                <div class="row">
                                    <div class="col-4">Humidity</div>
                                    <div class="col-4">{{item.humidity}}</div>
                                </div>
                                <div class="row">
                                    <div class="col-4">Wind speed</div>
                                    <div class="col-4">{{item.windSpeed}}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </b-list-group-item>
        </b-list-group>
    </div>
    <div v-else-if="weatherResultsError !== ''">
        <p>An error has occured - {{ weatherResultsError }}</p>
    </div>
</template>


<script lang="ts">
  import { Component, Vue, Prop, Provide } from 'vue-property-decorator';
  import IWeatherForecast from '../types/WeatherForecast';
  import dotenv from 'dotenv';
  

  @Component
  export default class VsResults extends Vue {

    @Prop() private weatherResults!: IWeatherForecast;
    @Prop() private weatherResultsError!: string;

  }


</script>

<style lang="scss" scoped>
  @import './vs-results.scss';
</style>
