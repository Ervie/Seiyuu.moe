<template>
<v-layout align-center justify-center class="container">
    <vue-particles color="#dedede" :particleOpacity="0.7" :particlesNumber="80" shapeType="circle" :particleSize="5" linesColor="#dedede"
      :linesWidth="1" :lineLinked="true" :lineOpacity="0.4" :linesDistance="150" :moveSpeed="4" :hoverEffect="true" hoverMode="repulse"
      :clickEffect="false" clickMode="push">
    </vue-particles>
    <v-card width="500px">
        <v-card-media src="microBig.jpg" height="300px" />
        <v-card-text>
            <h3 class="headline mb-0">
            <p class="text-xs-center"> {{ text }} </p>
            </h3>
        </v-card-text>
        <v-card-actions>
          <nuxt-link to="/Seiyuu">
            <v-btn outline color="primary">Compare Seiyuu</v-btn>
          </nuxt-link>
          <v-spacer />
          <nuxt-link to="/About">
            <v-btn small color="error">About</v-btn>
          </nuxt-link>
          <v-spacer />
          <nuxt-link to="/Anime">
            <v-btn outline color="primary">Compare Anime</v-btn>
          </nuxt-link>
        </v-card-actions>
    </v-card>
</v-layout>

</template>

<script scoped>
export default {
    layout: 'empty',
    data () {
        return {
          text: 'Lorem ipsum dolor sit amet, alienum intellegebat ad qui, at mea vero bonorum delicata. An his mucius voluptatibus. Et mei meis mucius comprehensam, vel ei suas definiebas. Altera corrumpit an eos. Nec dolore ponderum et. Eum epicurei disputando ei, nostrud feugait cu per.'
        }
    }
}
</script>

<style>
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
    z-index: 0;
}

.container {
    height: 100vh;
}

a {
    text-decoration: none;
}

</style>
