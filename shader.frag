#version 440

out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal;
in vec3 FragPos;


uniform vec3 lightPos;
uniform vec3 viewPos;


struct Material {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;
    float     shininess;
}; 

struct PointLight {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

struct DirectionalLight{
    vec3 direction;

    vec3 diffuse;
    vec3 specular;    
};

struct Spotlight{
    vec3 position;
    vec3 direction;
    float inner_corner;
    float outer_corner;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;

#define NR_POINT_LIGHTS 4  
uniform PointLight pointLights[NR_POINT_LIGHTS];

uniform DirectionalLight dirLight;
uniform Spotlight spotlight;

vec3 directionalLight(DirectionalLight dirLight, Material material, vec3 norm, vec3 viewDir)
{

    //diffuse
    vec3 lightDir = normalize(-dirLight.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 out_color = dirLight.diffuse * diff * vec3(texture(material.diffuse, texCoord)).rgb;

    //specular
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(reflectDir, viewDir), 0.0f), material.shininess);
    out_color += dirLight.specular * spec * texture(material.specular, texCoord).rgb;

    return out_color;
}

vec3 pointLight(PointLight light, Material material, vec3 norm, vec3 viewDir, vec3 FragPos)
{
    float distance = length(FragPos - light.position);
    float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    //diffuse
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 out_color = light.diffuse * diff * vec3(texture(material.diffuse, texCoord)) * attenuation;

    //specular
    float specularStrength = 0.5f;

    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    out_color += vec3(texture(material.specular, texCoord)) * spec * light.specular  * attenuation;

    return out_color; 
}

vec3 calcSpotlight(Spotlight light, Material material, vec3 norm, vec3 viewDir, vec3 FragPos)
{
    vec3 lightDir = normalize(spotlight.position - FragPos);
    float theta = dot(lightDir, normalize(-spotlight.direction));
    float diff = max(dot(norm, lightDir), 0.0);
    float I = (theta - spotlight.outer_corner)/(spotlight.inner_corner - spotlight.outer_corner);
    I = clamp(I, 0.0f, 1.0f);
    vec3 out_color = spotlight.diffuse * diff * vec3(texture(material.diffuse, texCoord)) * I;

    float specularStrength = 1.0f;
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    out_color += vec3(texture(material.specular, texCoord)) * spec * spotlight.specular * I;

    return out_color;   
}

void main()
{
    vec3 ambient = vec3(0.0f);
    vec3 diffuse = vec3(0.0f);
    vec3 specular = vec3(0.0f);

    vec3 out_color = vec3(0.0f);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 norm = normalize(Normal);




//    out_color += directionalLight(dirLight, material, norm, viewDir);

    for(int i = 0; i < NR_POINT_LIGHTS; i++)
    {
        out_color += pointLight(pointLights[i], material, norm, viewDir, FragPos);
        out_color += pointLights[i].ambient * vec3(texture(material.diffuse, texCoord));
    }

//    out_color += calcSpotlight(spotlight, material, norm, viewDir, FragPos);



    //out_color += vec3(texture(material.emission, texCoord)).rgb;

    outputColor = vec4(out_color, 1.0f);
}