<script>
/**
 * Widget-stat component -- specify the widget title, image, value, subtext and color of subtext.
 */
export default {
  props: {
    title: {
      type: String,
      default: ""
    },
    image: {
      type: String,
      default: ""
    },
    value: {
      type: String,
      default: ""
    },
    subText: {
      type: String,
      default: ""
    },
    color: {
      type: String,
      default: ""
    },
    path: {
      type: String,
      default: "/"
    },
    className: {
      type: String,
      default: ""
    },
    icon: {
      type: String,
      default: ""
    },
  },
  methods:{
    handleChangePage(path){
      this.$router.push(path);
    }
  }
};
</script>

<template>
  <div
      @click="handleChangePage(path)"
      class="card mini-stat"
      :class="`${className}`"
      style="cursor: pointer;"
  >
    <div class="card-body d-flex justify-content-between flex-column">
      <div class="mb-3 d-flex justify-content-start align-items-center">
        <div class="card-icon me-2">
          <div class="ico-item d-flex justify-content-center align-items-center">
            <i :class="`mdi ${icon} text-primary`"></i>
          </div>
        </div>
        <div class="card-title mb-0">
          <h5 class="font-size-16 mt-0 mb-0 text-white">{{title}}</h5>
        </div>
      </div>
      <div class="card-content d-flex justify-content-center align-items-center">
        <h4 class="fw-bold fs-1 text-white">
          {{value}}
        </h4>
      </div>
      <div class="card-content d-flex justify-content-end align-items-center">
        <i class="mdi mdi-arrow-right"></i>
      </div>
<!--      <div class="mb-4">-->
<!--&lt;!&ndash;        <div class="float-start mini-stat-img me-4">&ndash;&gt;-->
<!--&lt;!&ndash;          <img :src="`${image}`" alt />&ndash;&gt;-->
<!--&lt;!&ndash;        </div>&ndash;&gt;-->

<!--&lt;!&ndash;        <h4 class="fw-medium font-size-24">&ndash;&gt;-->
<!--&lt;!&ndash;          {{value}}&ndash;&gt;-->
<!--&lt;!&ndash;        </h4>&ndash;&gt;-->
<!--        <div :class="`mini-stat-label bg-mini-start bg-${color}`">-->
<!--          <h4 class="fw-medium font-size-24">-->
<!--            {{value}}-->
<!--          </h4>-->
<!--        </div>-->
<!--      </div>-->
    </div>
  </div>
</template>
<style>
.card-icon .ico-item{
  background-color: #f2f6fc;
  width: 30px;
  height: 30px;
  border-radius: 50px;
}
</style>
