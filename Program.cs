using System;
using System.Collections.Generic;
using System.IO;
using Massive;

namespace NuGetGalleryLocalPackageFileNormalizer
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            var model = new DynamicModel( args[0] );

            bool whatif = Array.IndexOf( args, "-whatif" ) > 0;

            IEnumerable<dynamic> packages = model.Query(
                    @"  SELECT pr.Id, p.[Key], p.Version, p.NormalizedVersion
                        FROM Packages p
                        INNER JOIN PackageRegistrations pr ON pr.[Key] = p.PackageRegistrationKey
                        WHERE p.NormalizedVersion IS NOT NULL AND p.Version != p.NormalizedVersion" );

            foreach ( dynamic package in packages )
            {
                string old = String.Format( "{0}.{1}.nupkg", package.Id.ToLower(), package.Version );

                string replacement = String.Format( "{0}.{1}.nupkg", package.Id.ToLower(), package.NormalizedVersion );

                Console.WriteLine( "Copy: {0} to {1}", old, replacement );

                if ( !whatif )
                {
                    File.Copy( Path.Combine( args[1], old ), Path.Combine( args[1], replacement ), true );
                }
            }
        }
    }
}
