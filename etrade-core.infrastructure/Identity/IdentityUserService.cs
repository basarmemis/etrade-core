using etrade_core.persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace etrade_core.infrastructure.Identity
{
    /// <summary>
    /// Identity ile entegrasyon için service interface'i
    /// </summary>
    public interface IIdentityUserService
    {
        /// <summary>
        /// Identity User'ı oluşturur
        /// </summary>
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);

        /// <summary>
        /// Identity User'ı günceller
        /// </summary>
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        /// <summary>
        /// Identity User'ı siler
        /// </summary>
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);

        /// <summary>
        /// Kullanıcı adına göre Identity User'ı bulur
        /// </summary>
        Task<ApplicationUser?> FindByUserNameAsync(string userName);

        /// <summary>
        /// E-posta adresine göre Identity User'ı bulur
        /// </summary>
        Task<ApplicationUser?> FindByEmailAsync(string email);
    }

    /// <summary>
    /// Identity ile entegrasyon için service implementasyonu
    /// </summary>
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityUserService> _logger;

        public IdentityUserService(UserManager<ApplicationUser> userManager, ILogger<IdentityUserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Identity User başarıyla oluşturuldu: {UserName}", user.UserName);
                }
                else
                {
                    _logger.LogError("Identity User oluşturulamadı: {UserName}, Hatalar: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity User oluşturulurken hata oluştu: {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Identity User başarıyla güncellendi: {UserName}", user.UserName);
                }
                else
                {
                    _logger.LogError("Identity User güncellenemedi: {UserName}, Hatalar: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity User güncellenirken hata oluştu: {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Identity User başarıyla silindi: {UserName}", user.UserName);
                }
                else
                {
                    _logger.LogError("Identity User silinemedi: {UserName}, Hatalar: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity User silinirken hata oluştu: {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<ApplicationUser?> FindByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
} 