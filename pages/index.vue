<template>
  <div id="mainContainer">
    <vue-particles color="#dedede" :particleOpacity="0.7" :particlesNumber="80" shapeType="circle" :particleSize="4" linesColor="#dedede"
      :linesWidth="1" :lineLinked="true" :lineOpacity="0.4" :linesDistance="150" :moveSpeed="3" :hoverEffect="true" hoverMode="grab"
      :clickEffect="true" clickMode="push">
    </vue-particles>
  </div>
</template>

<script>
export default {
    layout: 'empty'
}
</script>

<style>
#mainContainer {
    background-image: url("/static/poly.jpg");
    background-size: cover;
    width: 100%;
    height: 100%;
    position: absolute;
    top: 0;
    left: 0;
}
</style>
