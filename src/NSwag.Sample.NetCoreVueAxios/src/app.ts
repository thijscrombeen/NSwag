import { Component, Vue } from 'vue-property-decorator';
import Weather from './components/weather.vue';

@Component({
  components: {
    Weather,
  },
})
export default class App extends Vue {}
