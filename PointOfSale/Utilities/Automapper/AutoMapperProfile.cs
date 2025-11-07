using PointOfSale.Models;
using System.Globalization;
using PointOfSale.Model;
using AutoMapper;


namespace PointOfSale.Utilities.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {

          

          

            #region Category
            CreateMap<Category, VMCategory>()
            .ForMember(destiny =>
                destiny.IsActive,
                opt => opt.MapFrom(source => source.IsActive == true ? 1 : 0)
            );

            CreateMap<VMCategory, Category>()
            .ForMember(destiny =>
                destiny.IsActive,
                opt => opt.MapFrom(source => source.IsActive == 1 ? true : false)
            );
            #endregion

            #region Product
            CreateMap<Product, VMProduct>()
            .ForMember(destiny =>
                destiny.IsActive,
                opt => opt.MapFrom(source => source.IsActive == true ? 1 : 0)
            )
            .ForMember(destiny =>
                destiny.NameCategory,
                opt => opt.MapFrom(source => source.IdCategoryNavigation.Description)
            )
            .ForMember(destiny =>
                destiny.Price,
                opt => opt.MapFrom(source => Convert.ToString(source.Price.Value, new CultureInfo("es-PE")))
            )
            .ForMember(destiny =>
                destiny.PhotoBase64,
                opt => opt.MapFrom(source => Convert.ToBase64String(source.Photo))
            )
            .ForMember(destiny =>
                destiny.AllStock,
                opt => opt.MapFrom(source => source.AllStock)
                );

            CreateMap<VMProduct, Product>()
            .ForMember(destiny =>
                destiny.IsActive,
                opt => opt.MapFrom(source => source.IsActive == 1 ? true : false)
            )
            .ForMember(destiny =>
                destiny.IdCategoryNavigation,
                opt => opt.Ignore()
            )
            .ForMember(destiono =>
                destiono.Price,
                opt => opt.MapFrom(source => Convert.ToDecimal(source.Price, new CultureInfo("es-PE")))
            )
            .ForMember(destiny =>
                destiny.AllStock,
                opt => opt.MapFrom(source => source.AllStock)
            );

            #endregion

            #region TypeDocumentSale
            CreateMap<TypeDocumentSale, VMTypeDocumentSale>().ReverseMap();
            #endregion

            #region Sale
            CreateMap<Sale, VMSale>()
                .ForMember(destiny =>
                    destiny.TypeDocumentSale,
                    opt => opt.MapFrom(source => source.IdTypeDocumentSaleNavigation.Description)
                )
				
	             .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.User.Name))
	                .ForMember(dest => dest.TypeDocumentSale, opt => opt.MapFrom(src => src.IdTypeDocumentSaleNavigation.Description))

                .ForMember(destiny =>
                    destiny.Subtotal,
                    opt => opt.MapFrom(source => Convert.ToString(source.Subtotal.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.TotalTaxes,
                    opt => opt.MapFrom(source => Convert.ToString(source.TotalTaxes.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Total,
                    opt => opt.MapFrom(source => Convert.ToString(source.Total.Value, new CultureInfo("es-PE")))


                ).ForMember(destiny =>
                    destiny.AmountPaid,
                    opt => opt.MapFrom(source => Convert.ToString(source.AmountPaid.Value, new CultureInfo("es-PE")))

                ).ForMember(destiny =>
                    destiny.Change,
                    opt => opt.MapFrom(source => Convert.ToString(source.ChangeAmount.Value, new CultureInfo("es-PE")))
                ).ForMember(destiny =>
                    destiny.PaymentType,
                    opt => opt.MapFrom(source => source.PaymentType)


                ).ForMember(destiny =>
                    destiny.RegistrationDate,
                    opt => opt.MapFrom(source => source.RegistrationDate.Value.ToString("dd/MM/yyyy"))
                ).ForMember(dest => dest.DetailSales, opt => opt.MapFrom(src => src.DetailSales)); 



            CreateMap<VMSale, Sale>()
                .ForMember(destiny =>
                    destiny.Subtotal,
                    opt => opt.MapFrom(source => Convert.ToDecimal(source.Subtotal, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.TotalTaxes,
                    opt => opt.MapFrom(source => Convert.ToDecimal(source.TotalTaxes, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Total,
                    opt => opt.MapFrom(source => Convert.ToDecimal(source.Total, new CultureInfo("es-PE")))
                )
                // ADD THESE MISSING MAPPINGS:
                .ForMember(destiny =>
                    destiny.AmountPaid,
                    opt => opt.MapFrom(source => string.IsNullOrEmpty(source.AmountPaid) ? (decimal?)null : Convert.ToDecimal(source.AmountPaid, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.ChangeAmount,
                    opt => opt.MapFrom(source => string.IsNullOrEmpty(source.Change) ? (decimal?)null : Convert.ToDecimal(source.Change, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.PaymentType,
                    opt => opt.MapFrom(source => source.PaymentType)
                );

            #endregion

            #region DetailSale
            CreateMap<DetailSale, VMDetailSale>()
                .ForMember(destiny =>
                    destiny.DescriptionProduct,
                    opt => opt.MapFrom(source => source.DescriptionProduct) 
                )
                .ForMember(destiny =>
                    destiny.Price,
                    opt => opt.MapFrom(source => Convert.ToString(source.Price.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Total,
                    opt => opt.MapFrom(source => Convert.ToString(source.Total.Value, new CultureInfo("es-PE")))
                );

            CreateMap<VMDetailSale, DetailSale>()
                .ForMember(destiny =>
                    destiny.Price,
                    opt => opt.MapFrom(source => Convert.ToDecimal(source.Price, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Total,
                    opt => opt.MapFrom(source => Convert.ToDecimal(source.Total, new CultureInfo("es-PE")))
                );
            #endregion


            CreateMap<DetailSale, VMSalesReport>()
                .ForMember(destiny =>
                    destiny.RegistrationDate,
                    opt => opt.MapFrom(source => source.IdSaleNavigation.RegistrationDate.Value.ToString("dd/MM/yyyy"))
                )
                .ForMember(destiny =>
                    destiny.SaleNumber,
                    opt => opt.MapFrom(source => source.IdSaleNavigation.SaleNumber)
                )
                .ForMember(destiny =>
                    destiny.DocumentType,
                    opt => opt.MapFrom(source => source.IdSaleNavigation.IdTypeDocumentSaleNavigation.Description)
                )
                .ForMember(destiny =>
                    destiny.DocumentClient,
                    opt => opt.MapFrom(source => source.IdSaleNavigation.CustomerDocument)
                )
                .ForMember(destiny =>
                    destiny.ClientName,
                    opt => opt.MapFrom(source => source.IdSaleNavigation.ClientName)
                )
                .ForMember(destiny =>
                    destiny.SubTotalSale,
                    opt => opt.MapFrom(source => Convert.ToString(source.IdSaleNavigation.Subtotal.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.TaxTotalSale,
                    opt => opt.MapFrom(source => Convert.ToString(source.IdSaleNavigation.TotalTaxes.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.TotalSale,
                    opt => opt.MapFrom(source => Convert.ToString(source.IdSaleNavigation.Total.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Product,
                    opt => opt.MapFrom(source => source.DescriptionProduct)
                )
                .ForMember(destiny =>
                    destiny.Price,
                    opt => opt.MapFrom(source => Convert.ToString(source.Price.Value, new CultureInfo("es-PE")))
                )
                .ForMember(destiny =>
                    destiny.Total,
                    opt => opt.MapFrom(source => Convert.ToString(source.Total.Value, new CultureInfo("es-PE")))
                );

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destiny =>
                destiny.SubMenus,
                opt => opt.MapFrom(source => source.InverseIdMenuParentNavigation));
            #endregion Menu
        }
    }
}
