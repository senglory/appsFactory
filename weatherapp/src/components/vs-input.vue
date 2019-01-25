<template>
  <b-form inline class="d-flex justify-content-center">
    <!-- Bug in bootstrap-vue - need div around input or button disappears -->
    <div class="col-md-6 col-8 pl-0">
      <b-input v-model="cityOrPlz" class="w-100 mr-sm-2" type="text" placeholder="Enter City Name or PLZ"  />
    </div>
    <b-button class="my-2 my-sm-0" @click="search()">Search</b-button>
  </b-form>
</template>

<script lang="ts">
  import { Component, Vue, Provide } from 'vue-property-decorator';
  import dotenv from 'dotenv';

  @Component
  export default class VsInput extends Vue {

    public cityOrPlz: string = '';

    public search() {
      this.$emit('search', this.cityOrPlz);
    }
  }
</script>
