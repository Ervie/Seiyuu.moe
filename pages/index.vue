<template>
    <vue-particles color="#dedede" :particleOpacity="0.7" :particlesNumber="80" shapeType="circle" :particleSize="5" linesColor="#dedede"
      :linesWidth="1" :lineLinked="true" :lineOpacity="0.4" :linesDistance="150" :moveSpeed="4" :hoverEffect="true" hoverMode="repulse"
      :clickEffect="false" clickMode="push">
    </vue-particles>
</template>

<script>
export default {
    layout: 'empty'
}
</script>

<style scoped>
#particles-js {
    background-image: url("/static/poly.jpg");
    background-size: cover;
    background-repeat: no-repeat;
    overflow: hidden;
    width: 100%;
    height: 100%;
    position: absolute;
    top: 0;
    left: 0;
}
</style>
