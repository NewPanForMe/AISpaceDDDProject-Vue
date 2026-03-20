/**
 * 密码加密工具
 * 使用 AES-256-CBC 加密算法 (OpenSSL 兼容模式)
 */

import CryptoJS from 'crypto-js'

// 加密密钥（必须与后端一致）
const AES_SECRET_KEY = 'DDDProject2024SecretKey!'
const AES_IV = 'DDDProject2024IV!'

/**
 * AES 加密 (OpenSSL 兼容模式)
 * @param text 需要加密的文本
 * @returns 加密后的 Base64 字符串
 */
export function aesEncrypt(text: string): string {
  if (!text) return ''
  try {
    // 使用 OpenSSL 兼容模式，指定 key 和 iv
    const key = CryptoJS.enc.Utf8.parse(AES_SECRET_KEY)
    const iv = CryptoJS.enc.Utf8.parse(AES_IV)
    
    const encrypted = CryptoJS.AES.encrypt(text, key, {
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    })
    return encrypted.toString()
  } catch (error) {
    console.error('AES 加密失败:', error)
    return text
  }
}

/**
 * AES 解密 (OpenSSL 兼容模式)
 * @param encryptedText 加密的 Base64 字符串
 * @returns 解密后的文本
 */
export function aesDecrypt(encryptedText: string): string {
  if (!encryptedText) return ''
  try {
    const key = CryptoJS.enc.Utf8.parse(AES_SECRET_KEY)
    const iv = CryptoJS.enc.Utf8.parse(AES_IV)
    
    const decrypted = CryptoJS.AES.decrypt(encryptedText, key, {
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    })
    return CryptoJS.enc.Utf8.stringify(decrypted)
  } catch (error) {
    console.error('AES 解密失败:', error)
    return encryptedText
  }
}
