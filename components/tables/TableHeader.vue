<template>
<div >
<v-container fluid v-if="!avatarMode">
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
<v-container grid-list-xs fluid v-else>
    <v-layout row v-if="imageUrl">
        <v-flex xs6  offset-xs3 justify-center>
            <v-tooltip bottom>
                <img :src="imageUrl" slot="activator" class="miniav">
                <span>{{decodeHtml(text)}}</span>
            </v-tooltip>
        </v-flex>
    </v-layout>
    <v-layout v-else row>
        <v-flex md1 justify-center>
            <v-card-text class="title">{{decodeHtml(text)}}</v-card-text>
        </v-flex>
    </v-layout>
</v-container>
</div>
</template>

<script>
export default {
  name: 'TableHeader',
  props: ['imageUrl', 'text', 'avatarMode']
}
</script>

<style>

.customCenter {
    justify-content: center; /* center items vertically, in this case */
    align-items: center;     /* center items horizontally, in this case */
    text-align: center;
}

</style>
