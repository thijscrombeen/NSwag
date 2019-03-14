<template>
  <div class="hello">
    <table>
      <tr>
        <th>Date</th>
        <th>Temperature °C</th>
        <th>Temperature °F</th>
        <th>Summary</th>
      </tr>
      <tr v-for="forecast in forecasts">
        <td>{{forecast.dateFormatted}}</td>
        <td>{{forecast.temperatureC}}°C</td>
        <td>{{forecast.temperatureF}}°F</td>
        <td>{{forecast.summary}}</td>
      </tr>
    </table>
  </div>
</template>

<script lang="ts" src="./weather.ts">
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  h3 {
    margin: 40px 0 0;
  }

  ul {
    list-style-type: none;
    padding: 0;
  }

  li {
    display: inline-block;
    margin: 0 10px;
  }

  a {
    color: #42b983;
  }
</style>