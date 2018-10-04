<template>
<v-layout align-center justify-center class="container">
    <vue-particles color="#dedede" :particleOpacity="0.7" :particlesNumber="80" shapeType="circle" :particleSize="5" linesColor="#dedede"
      :linesWidth="1" :lineLinked="true" :lineOpacity="0.4" :linesDistance="150" :moveSpeed="4" :hoverEffect="true" hoverMode="repulse"
      :clickEffect="false" clickMode="push">
    </vue-particles>
    <v-card width="500px">
        <v-card-text primary-title>
            <h3 class="mb-0 styledHeader">
            <p class="text-xs-center" > {{ headerText }} </p>
            </h3>
        </v-card-text>
        <v-card-media src="microBig.jpg" height="300px" />
        <v-card-text>
            <h3 class="subheading mb-0">
            <p align="justify"> {{ text }} </p>
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
          headerText: 'What is Seiyuu.moe?',
          text: 'Did you ever wonder if those two seiyū worked together? This website allows you to select them from the list and find joint works between Japanese voice actors.'
        }
    }
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
    z-index: 0;
}

.container {
    height: 100vh;
}

a {
    text-decoration: none;
}

.styledHeader {
  font-family: 'Merienda', Georgia, 'Times New Roman', Times, cursive, serif;
  font-size: 28px;
}

</style>
