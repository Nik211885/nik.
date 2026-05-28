using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds demo travel trips for a selection of Vietnamese provinces. Idempotent — skips if any trips already exist.
/// </summary>
public static class TripSeeder
{
    /// <summary>Seeds sample trips using province codes as lookup keys.</summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Trips.AnyAsync()) return;

        var provinces = await db.Provinces.AsNoTracking().ToDictionaryAsync(p => p.Code);

        foreach (var (code, date, title, story) in GetTrips())
        {
            if (!provinces.TryGetValue(code, out var province)) continue;

            db.Trips.Add(new Trip
            {
                ProvinceId   = province.Id,
                Title        = title,
                Date         = DateOnly.Parse(date),
                Story        = story,
                CreatedDate  = DateTimeOffset.UtcNow,
                UpdatedDate  = DateTimeOffset.UtcNow,
            });
        }

        await db.SaveChangesAsync();
    }

    private static (string Code, string Date, string Title, string Story)[] GetTrips() =>
    [
        (
            "ha-noi", "2024-02-10", "Hà Nội mùa đông",
            "<p>Hà Nội mùa đông có một nét đẹp khác hẳn — sương mờ phủ mặt hồ Hoàn Kiếm, hơi lạnh len lỏi qua từng góc phố cổ. Đi bộ trên phố Đinh Tiên Hoàng buổi sáng sớm, nghe tiếng còi xe xa dần, cảm giác thành phố này vẫn đang thở.</p><p><br></p><p>Cốc cà phê trứng ở Đinh Tiên Hoàng nóng hổi, nhâm nhi nhìn ra hồ. Đó là những khoảnh khắc Hà Nội đẹp nhất mà tôi luôn muốn quay lại.</p>"
        ),
        (
            "ha-noi", "2023-09-15", "Phố đi bộ và ẩm thực vỉa hè",
            "<p>Cuối tuần ở phố đi bộ Hồ Hoàn Kiếm — nơi Hà Nội trở nên thật khác. Người ta đi bộ, ngồi chơi, nghe nhạc đường phố. Trẻ em chạy nhảy, người già ngồi tâm sự.</p><p><br></p><p>Bún chả Hàng Mành vẫn là địa chỉ ruột. Khói bếp than, mùi thịt nướng — cái thứ mùi đặc trưng của Hà Nội không lẫn vào đâu được.</p>"
        ),
        (
            "ho-chi-minh", "2024-03-20", "Sài Gòn không bao giờ ngủ",
            "<p>Lần đầu vào Sài Gòn vào lúc 11 giờ đêm — thành phố vẫn sáng rực và đông như ban ngày. Bùi Viện nhộn nhịp, Bến Thành rực đèn. Nhịp sống ở đây nhanh hơn Hà Nội rất nhiều, mọi người đi như chạy.</p><p><br></p><p>Sáng hôm sau ăn hủ tiếu Nam Vang ở góc đường Lê Thánh Tôn. Miền Nam có cái phóng khoáng riêng — người ta ngồi ăn sáng ngoài vỉa hè, mặt mày thư thái, không vội vàng. Thích cái đó lắm.</p>"
        ),
        (
            "da-nang", "2023-07-05", "Đà Nẵng — cầu Vàng và Ngũ Hành Sơn",
            "<p>Đà Nẵng vào mùa hè nắng gắt, nhưng biển Mỹ Khê vẫn đẹp đến nao lòng. Nước trong vắt, sóng vừa đủ để bơi. Ngồi nhìn ra biển lúc chiều tà, ánh mặt trời đổ xuống mặt nước — không thể không cầm máy ảnh lên.</p><p><br></p><p>Cầu Vàng trên Bà Nà Hills là điểm không thể bỏ qua. Hai bàn tay đá khổng lồ đỡ cây cầu trong mây — kỳ diệu hơn tưởng tượng. Cảnh ở đây giống một thế giới khác.</p>"
        ),
        (
            "quang-nam", "2023-07-07", "Hội An — đèn lồng và phố cổ",
            "<p>Từ Đà Nẵng xuôi về Hội An chỉ 30 phút nhưng như bước vào một chiều kích khác. Phố cổ vàng óng dưới nắng, đèn lồng đỏ rực. Buổi tối, thắp nến thả trên sông Hoài — khung cảnh đẹp đến mức không muốn về.</p><p><br></p><p>Cao lầu Hội An — sợi mì dày, thịt heo, tóp mỡ, rau sống — chỉ ngon đúng chuẩn khi ăn ở Hội An, vì nước giếng Bá Lễ nấu mới ra vị đó. Đi ăn hai lần trong một ngày cũng không chán.</p>"
        ),
        (
            "lao-cai", "2023-11-18", "Sa Pa trong mây",
            "<p>Lên Sa Pa vào tháng 11, sương mù đặc quánh. Đỉnh Fansipan ẩn hiện trong mây, bản làng người H'Mông lấp ló dưới thung lũng. Đi bộ trên ruộng bậc thang khi lúa đã gặt xong — những thửa ruộng trơ trọi nhưng vẫn có nét đẹp hoang vu riêng.</p><p><br></p><p>Đêm ngủ ở nhà homestay người Dao Đỏ. Ngồi bếp lửa, uống rượu ngô, nghe tiếng suối chảy xa xa. Cái lạnh -2 độ không còn đáng sợ nữa.</p>"
        ),
        (
            "quang-ninh", "2022-05-30", "Một đêm trên vịnh Hạ Long",
            "<p>Thuê tàu ngủ đêm trên vịnh Hạ Long — quyết định đúng đắn nhất của chuyến đi. Lúc sáng sớm 5 giờ leo lên boong tàu, sương mờ phủ kín mặt vịnh, những đảo đá vôi nhô lên như trong tranh thủy mặc. Không có tiếng gì ngoài tiếng nước vỗ mạn thuyền.</p><p><br></p><p>Chèo kayak vào hang Luồn — không gian im lặng tuyệt đối, chỉ có tiếng mái chèo và hơi thở. Một trong những trải nghiệm đẹp nhất tôi từng có.</p>"
        ),
        (
            "ninh-binh", "2022-10-08", "Tràng An nhìn từ thuyền",
            "<p>Đi thuyền trong Tràng An 3 tiếng đồng hồ — người chèo thuyền dùng chân, để hai tay tự do. Qua 9 hang động, mỗi hang một màu sắc khác nhau. Hang tối, hang sáng, hang có nhũ đá rủ xuống chạm gần mặt nước.</p><p><br></p><p>Cơm niêu Ninh Bình, dê núi nướng — bữa trưa ngon đến mức không muốn đứng dậy. Về đến Hà Nội mà còn nhớ mãi cái mùi khói lam chiều trên đồng Tam Cốc.</p>"
        ),
        (
            "thua-thien-hue", "2023-04-25", "Hoàng thành và bún bò Huế",
            "<p>Huế trầm mặc, chậm hơn mọi thành phố tôi từng đến. Đại Nội buổi chiều muộn, gần như chỉ có mình tôi. Bước qua từng cổng thành, nhìn những mái ngói rêu phong — lịch sử như đang thở.</p><p><br></p><p>Bún bò Huế ăn sáng ở góc đường Trần Phú, tô đỏ sậm, thịt bò mềm, cọng bún to. Húp một ngụm nước dùng — cay, ngọt, thơm sả. Miền Trung khắc khổ nhưng ẩm thực thì không thua ai.</p>"
        ),
        (
            "khanh-hoa", "2024-01-15", "Nha Trang — biển xanh và hải sản",
            "<p>Nha Trang vào tháng Giêng, ít khách du lịch hơn, biển sạch và yên tĩnh hơn hẳn mùa hè. Ngồi trên bãi Trần Phú nhìn ra khơi, nước biển xanh ngắt như được pha màu.</p><p><br></p><p>Tối ăn hải sản ở chợ Đầm — tôm hùm, cá mú, ốc hương. Giá mềm hơn nhiều so với các resort. Một bữa tối đáng nhớ với bạn bè, tiếng cười vang cả góc chợ.</p>"
        ),
        (
            "kien-giang", "2024-04-10", "Phú Quốc — đảo ngọc mùa hè",
            "<p>Phú Quốc không còn hoang sơ như trước nữa, nhưng bãi Sao vẫn giữ được nét đẹp gốc. Cát trắng mịn như bột, nước biển trong vắt nhìn thấy san hô bên dưới. Tắm xong nằm trên võng đung đưa, nghe sóng biển — quên hết mọi thứ.</p><p><br></p><p>Nước mắm Phú Quốc mua về làm quà — loại nguyên chất 40 độ đạm, mùi thơm nhẹ chứ không nồng. Cái thứ gia vị này làm món ăn ngon hơn rất nhiều.</p>"
        ),
        (
            "lam-dong", "2022-12-30", "Đà Lạt trong sương và mưa phùn",
            "<p>Đà Lạt vào dịp cuối năm, sương mù từ sáng đến tối. Mặc áo len ra phố, hít thở không khí lạnh trong lành thơm mùi hoa quỳ vàng. Thành phố cao nguyên này có cái gì đó rất Pháp — biệt thự cũ, rừng thông, hồ nhỏ.</p><p><br></p><p>Bánh mì xíu mại ăn sáng ở chợ Đà Lạt, ly cà phê chồn pha phin nhỏ giọt. Buổi chiều đạp xe quanh hồ Xuân Hương — gió lạnh thổi nhẹ, mặt hồ phẳng lặng. Khoảnh khắc bình yên hiếm có.</p>"
        ),
        (
            "binh-thuan", "2022-08-20", "Mũi Né — cồn cát đỏ và bình minh",
            "<p>Thức dậy lúc 5 giờ sáng để leo lên đồi cát đỏ Mũi Né xem bình minh. Cát đỏ rực dưới ánh mặt trời mọc, bóng người in dài trên cát — cảnh tượng tuyệt đẹp chỉ kéo dài 15 phút trước khi nắng lên gay gắt.</p><p><br></p><p>Thuê xe địa hình chạy quanh khu cát — ngồi sau mà tim đập thình thịch. Trượt cát xuống dốc bằng tấm ván trơn — thú vui đơn giản mà sảng khoái vô cùng.</p>"
        ),
        (
            "hai-phong", "2023-03-12", "Hải Phòng — bánh đa cua và Cát Bà",
            "<p>Hải Phòng gần Hà Nội nhưng không hay đến. Lần này nhân tiện ghé Cát Bà, dừng lại ăn sáng ở chợ Sắt. Bánh đa cua Hải Phòng — sợi bánh đa đỏ, cua đồng thật, cà chua, chả. Ăn một bát rồi gọi thêm bát nữa.</p><p><br></p><p>Cát Bà chiều tà, leo lên đỉnh pháo đài nhìn ra biển. Bên kia là vịnh Hạ Long, phía này là làng chài nhỏ. Hoàng hôn đổ màu da cam lên mặt nước — đẹp lặng người.</p>"
        ),
        (
            "ha-giang", "2023-10-02", "Hà Giang loop — 4 ngày trên cung đèo",
            "<p>Hà Giang loop là cung đường phượt đẹp nhất Việt Nam tôi từng đi. Đèo Mã Pì Lèng nhìn xuống vực sâu hàng trăm mét, sông Nho Quế xanh ngọc uốn lượn bên dưới. Đứng trên đèo mà chân run, tim đập, nhưng không thể rời mắt khỏi khung cảnh đó.</p><p><br></p><p>Mùa hoa tam giác mạch — những cánh đồng tím hồng trải rộng trên triền núi đá. Bản làng người Mông, trẻ em chạy chân đất, nụ cười thật. Những khoảnh khắc đó không máy ảnh nào chụp hết được.</p>"
        ),
        (
            "cao-bang", "2023-10-06", "Bản Giốc — thác nước hùng vĩ nhất Đông Nam Á",
            "<p>Sau Hà Giang, xuôi về Cao Bằng thêm một ngày đường. Thác Bản Giốc hiện ra đột ngột sau khúc cua — rộng hơn 300 mét, nước trắng xóa đổ xuống ba tầng, tiếng ầm vang xa cả cây số. Đứng trước thác mà choáng ngợp thật sự.</p><p><br></p><p>Vào hang Pắc Bó — nơi Bác Hồ từng sống và làm việc. Hang nhỏ, giản dị, nhưng bước vào đó có cảm giác lịch sử đang hiện diện. Suối Lê-nin trong vắt chảy quanh năm.</p>"
        ),
        (
            "can-tho", "2024-02-28", "Chợ nổi Cái Răng lúc bình minh",
            "<p>Xuất phát từ bến tàu lúc 5 giờ sáng, trời còn tối. Đến chợ nổi Cái Răng khi mặt trời vừa ló dạng, sương còn giăng trên mặt sông. Hàng chục chiếc thuyền chở trái cây, rau củ đủ màu sắc. Người bán buôn thúc xuồng khéo léo như đi trong mê cung.</p><p><br></p><p>Ăn bún mắm trên thuyền — tô nước dùng đậm đà, cá, tép, rau thơm. Cái trải nghiệm ăn sáng trên sông nước đó không đâu có được.</p>"
        ),
        (
            "son-la", "2022-03-15", "Mộc Châu mùa hoa cải vàng",
            "<p>Tháng 3 là mùa hoa cải vàng ở Mộc Châu. Những thửa ruộng bậc thang chuyển sang màu vàng rực, tương phản với núi non xanh thẫm phía sau. Dừng xe giữa đường mà không muốn đi nữa.</p><p><br></p><p>Đêm ngủ ở bản Thái, ăn cơm lam, gà đồi nướng. Sáng dậy sớm đi trong sương, hái dâu tây ngay trên nương — ngọt tự nhiên, không cần rửa cũng ăn được. Mộc Châu là nơi tôi muốn quay lại nhất.</p>"
        ),
        (
            "quang-binh", "2023-06-18", "Phong Nha — vương quốc hang động",
            "<p>Hang Phong Nha có tuổi đời 400 triệu năm. Đi thuyền vào trong hang, đèn pha rọi lên những nhũ đá khổng lồ — cam, vàng, trắng đủ màu. Tiếng nước nhỏ giọt vang vọng, không khí mát lạnh bất ngờ dù ngoài trời 40 độ.</p><p><br></p><p>Hang Tối — cắm đầu đèn đội đầu, leo trèo qua đá, bơi qua những đoạn nước. Trải nghiệm cần sức lực nhưng đáng từng giây. Nhìn lên trần hang tối đen, nghe tĩnh lặng tuyệt đối.</p>"
        ),
        (
            "dak-lak", "2022-01-20", "Buôn Ma Thuột — cà phê và đất đỏ",
            "<p>Buôn Ma Thuột là thủ phủ cà phê của Việt Nam. Vườn cà phê Trung Nguyên rộng mênh mông, hàng cây cao su thẳng tắp. Uống cà phê ngay tại nông trường — đắng, thơm, mạnh hơn cà phê ở Hà Nội nhiều lần.</p><p><br></p><p>Ghé buôn làng người Ê-đê, nghe cồng chiêng Tây Nguyên. Âm thanh trầm hùng, ngân vang trong đêm bên bếp lửa. Một thứ âm nhạc gắn với đất trời, không thể nghe qua loa mà phải nghe tại chỗ.</p>"
        ),
        (
            "ba-ria-vung-tau", "2024-05-04", "Vũng Tàu — cuối tuần bên biển",
            "<p>Vũng Tàu cách Sài Gòn 2 tiếng xe — điểm đến cuối tuần hoàn hảo của dân thành phố. Bãi Sau ít đông hơn bãi Trước, sóng to hơn, hợp để bơi. Buổi chiều thuê xe đạp đạp dọc bờ biển, gió mát, tâm trạng nhẹ nhõm.</p><p><br></p><p>Leo lên Bạch Dinh — biệt thự cổ thời Pháp thuộc. Đứng trên đồi nhìn xuống thành phố và biển xanh — khung cảnh như trong bưu thiếp cũ. Cá lóc nướng trui ăn tối, hải sản tươi sống, bia lạnh — chuyến đi không cần phức tạp hơn thế.</p>"
        ),
        (
            "dien-bien", "2022-09-02", "Điện Biên Phủ — hành trình về lịch sử",
            "<p>Đến Điện Biên Phủ nhân dịp kỷ niệm Quốc khánh. Thung lũng Mường Thanh rộng mênh mông nhìn từ đồi A1 — bây giờ là đồng ruộng xanh bình yên, khó tưởng tượng 70 năm trước là chiến trường ác liệt nhất lịch sử.</p><p><br></p><p>Đến bảo tàng Chiến thắng Điện Biên Phủ, xem những hiện vật còn lại. Nghĩa trang liệt sĩ A1 — hàng ngàn ngôi mộ trắng tinh nằm trên đồi. Đứng ở đó không nói được lời nào.</p>"
        ),
        (
            "nghe-an", "2023-08-15", "Nghệ An — quê hương Bác Hồ",
            "<p>Dừng lại ở Nam Đàn ghé thăm Làng Sen — quê nội Bác Hồ. Ngôi nhà tranh đơn sơ, vườn cây xanh mát. Nghĩ đến tuổi thơ của một người sau này thay đổi cả dân tộc mà thấy xúc động.</p><p><br></p><p>Bún bò Nghệ An đậm đà hơn bún bò Huế, thêm lạc rang. Ăn một bát nóng hổi bên đường, trời mưa bên ngoài — bình dị mà ấm lòng.</p>"
        ),
        (
            "thanh-hoa", "2022-07-20", "Sầm Sơn — biển xanh miền Bắc",
            "<p>Sầm Sơn là bãi biển gần nhất của người miền Bắc. Nước không trong bằng miền Nam nhưng sóng to, bơi rất thích. Mùa hè đông nghịt người — trải dài trên bãi cát là hàng trăm chiếc ô đủ màu sắc.</p><p><br></p><p>Chả mực Thanh Hóa — miếng mực giã nhuyễn, dai giòn, ăn với bia hơi. Buổi tối ngồi ở quán vỉa hè nhìn ra biển đêm, nghe sóng vỗ — giản dị nhưng đó là những khoảnh khắc tôi nhớ nhất.</p>"
        ),
        (
            "hung-yen", "2023-01-28", "Hưng Yên — nhãn lồng và đền Chử Đồng Tử",
            "<p>Tết xong ghé Hưng Yên theo đường sông. Mùa nhãn chưa đến nhưng vườn nhãn đã xanh um. Đền Chử Đồng Tử bên sông Hồng — một trong tứ bất tử của người Việt. Khói hương thơm ngát, người đi lễ đầu năm tấp nập.</p><p><br></p><p>Bánh cuốn Phố Hiến ăn buổi sáng — mỏng tang, nhân thịt, rưới nước mắm pha loãng với hành phi. Những góc phố Phố Hiến cổ còn giữ được nét buồn tĩnh lặng của thương cảng xưa.</p>"
        ),
        (
            "bac-ninh", "2024-03-05", "Bắc Ninh — quan họ và chùa Phật Tích",
            "<p>Hội Lim tháng Giêng — những liền anh liền chị áo dài khăn đóng hát quan họ trên thuyền giữa ao làng. Làn điệu ngọt ngào, da diết. Đứng bên bờ nghe mà thấy lòng lâng lâng khó tả.</p><p><br></p><p>Chùa Phật Tích trên núi Lạn Kha. Tượng Phật bằng đá từ thế kỷ XI vẫn còn nguyên vẹn. Leo lên cao nhìn xuống đồng bằng Bắc Bộ trải rộng tít tắp. Cái bình yên của vùng đất ngàn năm văn hiến.</p>"
        ),
    ];
}
