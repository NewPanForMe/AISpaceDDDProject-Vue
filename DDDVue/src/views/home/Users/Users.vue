<template>
  <div class="users-page">
    <h2 class="page-title">用户管理</h2>
    <div class="users-content">
      <el-card>
        <p class="placeholder-text">用户管理功能开发中...</p>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.users-page {
  padding: 20px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin-bottom: 20px;
}

.users-content {
  height: calc(100vh - 150px);
}

.placeholder-text {
  text-align: center;
  color: #666;
  font-size: 16px;
  padding: 40px;
}
</style>
