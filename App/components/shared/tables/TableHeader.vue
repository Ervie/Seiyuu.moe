<template>
<div >
<v-container fluid>
    <v-layout row v-if="imageUrl">
        <v-flex class="customCenter">
            <v-tooltip bottom >
                <img :src="imageUrl" slot="activator" class="av">
                <span>{{decodeHtml(text)}}</span>
            </v-tooltip>
        </v-flex>
    </v-layout>
    <v-layout v-else row>
        <v-flex class="customCenter">
            <v-card-text class="display-1">{{decodeHtml(text)}}</v-card-text>
        </v-flex>
    </v-layout>
</v-container>
</div>
</template>

<script>
export default {
  name: 'TableHeader',
  props: {
    imageUrl: {
        type: String,
        required: false
    },
    text: {
        type: String,
        required: true
    }
  }
}
</script>

<style>

.customCenter {
    justify-content: center; /* center items vertically, in this case */
    align-items: center;     /* center items horizontally, in this case */
    text-align: center;
}

</style>
