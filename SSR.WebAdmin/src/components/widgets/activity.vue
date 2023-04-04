<script>
/**
 * Activity component
 */
export default {
  props: {
    title: {
      type: String,
      default: "Đang hoạt động"
    },
    activityUserData: {
      type: Array,
      default: function() {
        return [];
      }
    }
  }
};
</script>

<template>
  <div class="card">
    <div class="card-body">
      <h4 class="card-title mb-4 text-primary">{{title}}</h4>
      <div class="scrollbar">
        <ol class="activity-feed force-overflow">
          <li class="feed-item border-0" v-for="activity in activityUserData" :key="activity.id">
            <div class="feed-item-list">
              <span class="date text-dark fw-bold">{{activity.contentLog}}</span>
              <span class="activity-text"><i class="mdi mdi-account-clock me-1 text-warning"></i>{{activity.createdAtShow}} {{activity.createdAtTimeShow}}</span>
            </div>
          </li>
        </ol>
      </div>
<!--      <div class="text-center">-->
<!--        <a href="#" class="btn btn-primary">Load More</a>-->
<!--      </div>-->
    </div>
    <!-- end card-body -->
  </div>
  <!-- end card -->
</template>

<style>

.scrollbar
{
  float: left;
  height: 400px;
  width: 100%;
  overflow-y: scroll;
}

.force-overflow
{
  min-height: 300px;
}
.scrollbar::-webkit-scrollbar-track
{
  -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);
  background-color: #F5F5F5;
}

.scrollbar::-webkit-scrollbar
{
  width: 3px;
  background-color: #f2f6fc;
}

.scrollbar::-webkit-scrollbar-thumb
{
  background-color: #f5f5f5;
}
</style>
