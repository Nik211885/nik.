using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds all 63 Vietnamese provinces on startup. Idempotent — skips if any province already exists.
/// Province names match the GeoJSON <c>NAME_1</c> property used by the Leaflet map.
/// </summary>
public static class ProvinceSeeder
{
    /// <summary>
    /// Ensures all 63 Vietnamese provinces are present in the database.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Provinces.AnyAsync()) return;

        db.Provinces.AddRange(GetProvinces().Select(p => new Province
        {
            Name = p.Name,
            Code = p.Code,
        }));

        await db.SaveChangesAsync();
    }

    private static (string Name, string Code)[] GetProvinces() =>
    [
        ("An Giang",                  "an-giang"),
        ("Bà Rịa - Vũng Tàu",        "ba-ria-vung-tau"),
        ("Bắc Giang",                 "bac-giang"),
        ("Bắc Kạn",                   "bac-kan"),
        ("Bạc Liêu",                  "bac-lieu"),
        ("Bắc Ninh",                  "bac-ninh"),
        ("Bến Tre",                   "ben-tre"),
        ("Bình Định",                 "binh-dinh"),
        ("Bình Dương",                "binh-duong"),
        ("Bình Phước",                "binh-phuoc"),
        ("Bình Thuận",                "binh-thuan"),
        ("Cà Mau",                    "ca-mau"),
        ("Cần Thơ",                   "can-tho"),
        ("Cao Bằng",                  "cao-bang"),
        ("Đà Nẵng",                   "da-nang"),
        ("Đắk Lắk",                   "dak-lak"),
        ("Đắk Nông",                  "dak-nong"),
        ("Điện Biên",                 "dien-bien"),
        ("Đồng Nai",                  "dong-nai"),
        ("Đồng Tháp",                 "dong-thap"),
        ("Gia Lai",                   "gia-lai"),
        ("Hà Giang",                  "ha-giang"),
        ("Hà Nam",                    "ha-nam"),
        ("Hà Nội",                    "ha-noi"),
        ("Hà Tĩnh",                   "ha-tinh"),
        ("Hải Dương",                 "hai-duong"),
        ("Hải Phòng",                 "hai-phong"),
        ("Hậu Giang",                 "hau-giang"),
        ("Hòa Bình",                  "hoa-binh"),
        ("Hưng Yên",                  "hung-yen"),
        ("Khánh Hòa",                 "khanh-hoa"),
        ("Kiên Giang",                "kien-giang"),
        ("Kon Tum",                   "kon-tum"),
        ("Lai Châu",                  "lai-chau"),
        ("Lâm Đồng",                  "lam-dong"),
        ("Lạng Sơn",                  "lang-son"),
        ("Lào Cai",                   "lao-cai"),
        ("Long An",                   "long-an"),
        ("Nam Định",                  "nam-dinh"),
        ("Nghệ An",                   "nghe-an"),
        ("Ninh Bình",                 "ninh-binh"),
        ("Ninh Thuận",                "ninh-thuan"),
        ("Phú Thọ",                   "phu-tho"),
        ("Phú Yên",                   "phu-yen"),
        ("Quảng Bình",                "quang-binh"),
        ("Quảng Nam",                 "quang-nam"),
        ("Quảng Ngãi",                "quang-ngai"),
        ("Quảng Ninh",                "quang-ninh"),
        ("Quảng Trị",                 "quang-tri"),
        ("Sóc Trăng",                 "soc-trang"),
        ("Sơn La",                    "son-la"),
        ("Tây Ninh",                  "tay-ninh"),
        ("Thái Bình",                 "thai-binh"),
        ("Thái Nguyên",               "thai-nguyen"),
        ("Thanh Hóa",                 "thanh-hoa"),
        ("Thừa Thiên Huế",            "thua-thien-hue"),
        ("Tiền Giang",                "tien-giang"),
        ("Trà Vinh",                  "tra-vinh"),
        ("Tuyên Quang",               "tuyen-quang"),
        ("Thành phố Hồ Chí Minh",    "ho-chi-minh"),
        ("Vĩnh Long",                 "vinh-long"),
        ("Vĩnh Phúc",                 "vinh-phuc"),
        ("Yên Bái",                   "yen-bai"),
    ];
}
