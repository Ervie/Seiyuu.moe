<template>
<div>
  <v-container hidden-md-and-down>
    <v-timeline v-if="items != null">
      <v-timeline-item
        v-for="(anime,i) in items"
        :key="i"
        color="accent"
      >
        <span slot="opposite" class="title"> {{ anime.aired_date }} </span>
        <v-card>
          <v-layout align-center justify-center row fill-height v-if="i % 2 === 0">
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
            <v-flex xs4>
              <v-layout align-center justify-center>
                <v-img :src="pathToImage(anime.image_url)" width="145px" height="210px"/>
              </v-layout>
            </v-flex>
          </v-layout>
          <v-layout align-center justify-center row fill-height v-else>
            <v-flex xs4>
              <v-layout align-center justify-center>
               <v-img :src="pathToImage(anime.image_url)" width="145px" height="210px"/>
              </v-layout>
            </v-flex>
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
          </v-layout>
        </v-card>
      </v-timeline-item>
    </v-timeline>
  </v-container>
  <v-container hidden-lg-and-up>
    <v-timeline
      align-top
      dense>
      <v-timeline-item
        v-for="(anime,i) in items"
        :key="i"
        color="accent"
      >
        <v-layout align-center justify-center row fill-height >
            <v-flex xs4>
        <div class="caption" > {{ anime.aired_date }} </div>
            </v-flex>
            <v-flex xs8>
        <div class="subheading"> {{ anime.name }}  </div>
            </v-flex>
        </v-layout>
      </v-timeline-item>
    </v-timeline>
  </v-container>
</div>
</template>

<script>
export default {
    name: 'SeiyuuTimeline',
    props: {
        items: {
          type: Array,
          required: false,
          default: () => {
            return [];
          }
        }
    }
}
</script>

<style>
  .timeline-dot {
    width: 144px;
    height: 144px;
  }
</style>


