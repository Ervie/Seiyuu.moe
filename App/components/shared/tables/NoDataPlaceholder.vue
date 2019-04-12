<template>
    <v-alert v-if="!isLoadingData" :value="true" color="error" icon="warning">
        Sorry, nothing to display here :(
        </v-alert>
        <v-alert v-else :value="true" color="info" icon="help">
        Loading data, please wait...
    </v-alert>
</template>

<script>
export default {
    name: 'NoDataPlaceholder',
    props: {
        isLoadingData: {
            type: Boolean,
            required: true
        }
    }
}
</script>

