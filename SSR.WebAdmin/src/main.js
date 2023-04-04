import Vue from 'vue'
import App from './App.vue'
import BootstrapVue from 'bootstrap-vue'
import Vuelidate from 'vuelidate'
import VueMask from 'v-mask'
import * as VueGoogleMaps from 'vue2-google-maps'
import VueSweetalert2 from 'vue-sweetalert2'
import VueApexCharts from 'vue-apexcharts'
import router from './router/index'
import store from '@/state/store'
import vco from "v-click-outside"
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import FunctionalCalendar from 'vue-functional-calendar';
//import CKEditor from '@ckeditor/ckeditor5-vue2';
import CKEditor from 'ckeditor4-vue'
// Toast
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
import VueLoading from 'vue-loading-overlay';
import 'vue-loading-overlay/dist/vue-loading.css';
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import VJstree from 'vue-jstree'
import "@/design/index.scss";
import 'dtoaster/dist/dtoaster.css'
import DToaster from 'dtoaster'

import Sparkline from 'vue-sparklines'
import VueCropper from "vue-cropper"

Vue.config.productionTip = false
const options = {
  // You can set your default options here
};
Vue.use(DToaster, {
  position: 'top-right', //toasts container position on the screen
  containerOffset: '45px', //toasts container offset from top/bottom of the screen
})
Vue.use(VueCropper)
Vue.use(Toast, options);
Vue.use(BootstrapVue);
Vue.use(VJstree)
Vue.use(Vuelidate);
Vue.use(VueMask)
Vue.use(VueSweetalert2)
Vue.use(require('vue-chartist'))
Vue.use(vco)
Vue.use(Sparkline)
Vue.use(VueLoading, {
  canCancel: false,
  color: '#000000',
  loader: 'spinner',
  width: 30,
  height: 30,
  backgroundColor: '#ffffff',
  opacity: 0.7,
  zIndex: 999,
}, {

});
Vue.use(VueGoogleMaps, {
  load: {
    key: 'AIzaSyAbvyBxmMbFhrzP9Z8moyYr6dCr-pzjhBE',
    libraries: 'places',
  },
  installComponents: true
})
Vue.component('apexchart', VueApexCharts)
Vue.use( CKEditor );
Vue.use(FunctionalCalendar, {
  dayNames: ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su']
});

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app')

Vue.filter('truncate', function (value, len = 50) {
  if (value) {
    if (value.length > len) {
      value = value.substring(0, len) + '...'
    }
    return value
  }
})
