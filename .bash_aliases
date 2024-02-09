ef() {
    dotnet ef "$@" --msbuildprojectextensionspath ./.build/obj/
}