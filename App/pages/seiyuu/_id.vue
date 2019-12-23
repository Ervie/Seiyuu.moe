<template>
  <div> 
    <v-flex xs12 v-if="profileData">
      <v-layout row>
        <v-flex md2 xs4>
          <v-img
            :src="pathToImage(this.profileData.imageUrl)"
            height="350px"
            contain
          ></v-img>
        </v-flex>
        <v-flex md10 xs8>
          <div>
            <div class="display-1">{{ profileData.name }}</div>
            <v-card-text>
              <p class="text-sm-left headline">
                <b>Given name:</b> {{ profileData.japaneseName }}
              </p>
              <p class="text-sm-left headline">
                <b>Birthday:</b> {{ formatDate(profileData.birthday) }}
              </p>
            </v-card-text>
          </div>
        </v-flex>
      </v-layout>
    </v-flex>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'SeiyuuProfilePage',
  data() {
    return {
      profileData: null
    }
  },
  async asyncData ({ error, store, params }) {
    return axios.get(process.env.apiUrl + '/api/seiyuu/' + params.id)
    .then((response) => {
      if (response == null && !response.error) {
        error({ statusCode: 404, message: 'Post not found' });
      }
      return { 
        profileData: response.data.payload
      }
    })
    .catch((e) => {
      console.log(e);
      error({ statusCode: 404, message: 'Post not found' });
    })
  }
}
</script>