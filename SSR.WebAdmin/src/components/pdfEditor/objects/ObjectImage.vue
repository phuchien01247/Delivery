<script>
import {mapState} from "vuex";

export default {
  name: "ObjectImage",
  props: {
    operation: {required: true}
  },
  data() {
    return {
      signatureContainer: null,
      data:{
        x: 0,
        y: 0,
      }
    }
  },
  mounted() {
     this.setupEvent()
  },
  computed:{
    moveOperation: function () {
      return this.operation === "move"
    },
    ...mapState('pdfEditorStore', ['scale'])
  },
  methods: {
    setupEvent() {
      this.$refs.signatureContainer.addEventListener('mousedown', this.handleMousedown)
      this.$refs.signatureContainer.addEventListener('touchstart', this.handleTouchStart)
    },

    handleMousedown(event) {
     this.data.x = event.clientX
      this.data.y = event.clientY
      const target = event.target
      this.$emit('panstart', {
        x:   this.data.x,
        y:   this.data.y,
        target,
        currentTarget:   this.$refs.signatureContainer
      })
      window.addEventListener('mousemove', this.handleMousemove)
      window.addEventListener('mouseup', this.handleMouseup)
    },

    handleMousemove(event) {
      const dx = event.clientX -   this.data.x
      const dy = event.clientY -   this.data.y
      this.data.x = event.clientX
      this.data.y = event.clientY
      this.$emit('panmove', {
        x:   this.data.x,
        y:   this.data.y,
        dx,
        dy
      })
    },

    handleMouseup(event) {
      this.data.x = event.clientX
      this.data.y = event.clientY

      this.$emit('panend', {x:   this.data.x, y:   this.data.y})

      window.removeEventListener('mousemove', this.handleMousemove)
      window.removeEventListener('mouseup', this.handleMouseup)
    },

    handleTouchStart(event) {
      if (event.touches.length > 1) return
      const touch = event.touches[0]
      this.data.x = touch.clientX
      this.data.y = touch.clientY
      const target = touch.target

      this.$emit('panstart', {x:   this.data.x, y:   this.data.y, target})
      window.addEventListener('touchmove',   this.handleTouchmove,{ passive: false }) // { passive: false }
      window.addEventListener('touchend',   this.handleTouchend)
    },

    handleTouchmove(event) {
      console.log(event, "move")
      event.preventDefault()
      if (event.touches.length > 1) return;
      const touch = event.touches[0]
      const dx = touch.clientX -   this.data.x
      const dy = touch.clientY -   this.data.y
      this.data.x = touch.clientX
      this.data.y = touch.clientY

      this.$emit('panmove', {x:   this.data.x, y:   this.data.y, dx, dy})
    },

    handleTouchend(event) {
      event.preventDefault()
      const touch = event.changedTouches[0]
      this.data.x = touch.clientX
      this.data.y = touch.clientY

      this.$emit('panend', {x:   this.data.x, y:   this.data.y})
      window.removeEventListener('touchmove',   this.handleTouchmove)
      window.removeEventListener('touchend',   this.handleTouchend)
    },
  }
}
</script>
<template>
  <div   ref="signatureContainer"
       :class="['tw-absolute tw-w-full tw-h-full tw-cursor-grab btn-size-grab',
            { 'tw-cursor-grabbing': moveOperation }]">
    <div data-direction="left"
         class="resize-border tw-h-full tw-w-1 tw-left-0 tw-top-0 tw-border-l cursor-ew-resize" />
    <div data-direction="top"
         class="resize-border tw-w-full tw-h-1 tw-left-0 tw-top-0 tw-border-t cursor-ns-resize" />
    <div data-direction="bottom"
         class="resize-border tw-w-full tw-h-1 tw-left-0 tw-bottom-0 tw-border-b cursor-ns-resize" />
    <div data-direction="right"
         class="resize-border tw-h-full tw-w-1 tw-right-0 tw-top-0 tw-border-r cursor-ew-resize" />
    <div data-direction="left-top"
         class=" tw-left-0 tw-top-0 cursor-nwse-resize transform-translate-x-1/2 tw--translate-y-1/2 md:scale-25" />
    <div data-direction="right-top"
         class=" tw-right-0 tw-top-0 cursor-nesw-resize tw-transform
                tw-translate-x-1/2 tw--translate-y-1/2 md:scale-25" />
    <div data-direction="left-bottom"
         class=" tw-left-0 tw-bottom-0 cursor-nesw-resize tw-transform
                tw--translate-x-1/2 tw-translate-y-1/2 md:scale-25" />
    <div data-direction="right-bottom"
         class="resize-corner tw-right-0 tw-bottom-0 cursor-nwse-resize tw-transform
            tw-translate-x-1/2 tw-translate-y-1/2 md:scale-25"/>
  </div>
</template>
<style>
    .operation {
        background-color: rgba(0, 0, 0, 0.3)
    }
    .resize-border {
        position: absolute;
        border-style: dashed;
        border-color: #718096;
    }
    .resize-corner {
        position: absolute;
        width: .9rem;
        height: .9rem;
        background-color: #3475E0;
      cursor: nwse-resize;
    }
    .tw-cursor-grabbing{
      background-color: rgba(0, 0, 0, 0.3);
      cursor: grabbing !important;
    }

    .tw-cursor-grab{
      cursor: grab;
    }
</style>
